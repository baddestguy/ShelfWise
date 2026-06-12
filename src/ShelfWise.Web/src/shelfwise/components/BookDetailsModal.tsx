import { useEffect, useState } from 'react'
import type { Book } from '../types'

type BookDetailsModalProps = {
  book: Book | null
  canManageBooks: boolean
  canDeleteBooks: boolean
  onEdit: (book: Book) => void
  onDelete: (book: Book) => void
  onOpenCirculation: (mode: 'checkout' | 'checkin', book: Book) => void
  onClose: () => void
}

export function BookDetailsModal({
  book,
  canManageBooks,
  canDeleteBooks,
  onEdit,
  onDelete,
  onOpenCirculation,
  onClose
}: BookDetailsModalProps) {
  const [coverUrl, setCoverUrl] = useState<string | null>(null)
  const [coverLoading, setCoverLoading] = useState(false)
  const [imageLoading, setImageLoading] = useState(false)

  useEffect(() => {
    if (!book) {
      setCoverUrl(null)
      setCoverLoading(false)
      setImageLoading(false)
      return
    }

    setCoverUrl(null)
    setImageLoading(false)
    const cacheKey = `shelfwise-cover:${book.title.toLowerCase()}|${book.author.toLowerCase()}`
    const cached = window.localStorage.getItem(cacheKey)
    if (cached) {
      setCoverUrl(cached === 'none' ? null : cached)
      setCoverLoading(false)
      return
    }

    const controller = new AbortController()

    async function loadCover() {
      setCoverLoading(true)
      try {
        const params = new URLSearchParams({
          title: book.title,
          author: book.author,
          fields: 'title,author_name,cover_i',
          limit: '5'
        })
        const response = await fetch(`https://openlibrary.org/search.json?${params}`, {
          signal: controller.signal
        })
        if (!response.ok) throw new Error('Cover lookup failed.')

        const payload = await response.json() as OpenLibrarySearchResponse
        const match = pickBestCover(payload.docs, book)
        const nextCoverUrl = match?.cover_i
          ? `https://covers.openlibrary.org/b/id/${match.cover_i}-L.jpg?default=false`
          : null

        window.localStorage.setItem(cacheKey, nextCoverUrl ?? 'none')
        setCoverUrl(nextCoverUrl)
        setImageLoading(Boolean(nextCoverUrl))
      } catch (error) {
        if (!controller.signal.aborted) {
          window.localStorage.setItem(cacheKey, 'none')
          setCoverUrl(null)
        }
      } finally {
        if (!controller.signal.aborted) setCoverLoading(false)
      }
    }

    loadCover()

    return () => controller.abort()
  }, [book])

  if (!book) return null

  return (
    <div className="modal-backdrop" role="presentation">
      <section className="modal detail-modal" role="dialog" aria-modal="true">
        <div className="book-detail-layout">
          {coverUrl ? (
            <div className="book-cover-frame">
              {imageLoading && <span className="cover-spinner" aria-label="Loading cover" />}
              <img
                className="book-cover-image"
                src={coverUrl}
                alt={`${book.title} cover`}
                onLoad={() => setImageLoading(false)}
                onError={() => {
                  setImageLoading(false)
                  setCoverUrl(null)
                }}
              />
            </div>
          ) : (
            <div className="book-cover-placeholder" aria-hidden="true">
              {coverLoading ? <span className="cover-spinner" /> : <span>{book.title.slice(0, 1).toUpperCase()}</span>}
            </div>
          )}

          <div className="book-detail-content">
            <div>
              <p className="detail-kicker">{book.category}</p>
              <h2>{book.title}</h2>
              <p>{book.author}</p>
            </div>

            <dl className="detail-grid">
              <div>
                <dt>Genre</dt>
                <dd>{book.genre || 'Unassigned'}</dd>
              </div>
              <div>
                <dt>Total Copies</dt>
                <dd>{book.totalCopies}</dd>
              </div>
              <div>
                <dt>Checked Out</dt>
                <dd>{book.checkedOutCopies}</dd>
              </div>
              <div>
                <dt>Available</dt>
                <dd>{book.availableCopies}</dd>
              </div>
            </dl>

            <div className="modal-actions detail-actions">
              {canManageBooks && (
                <>
                  <button type="button" className="secondary" onClick={() => onEdit(book)}>Edit</button>
                  {book.availableCopies > 0 && (
                    <button type="button" onClick={() => onOpenCirculation('checkout', book)}>Check Out</button>
                  )}
                  {book.checkedOutCopies > 0 && (
                    <button type="button" className="secondary" onClick={() => onOpenCirculation('checkin', book)}>Check In</button>
                  )}
                </>
              )}
              {canDeleteBooks && <button type="button" className="danger" onClick={() => onDelete(book)}>Delete</button>}
              <button type="button" className="secondary" onClick={onClose}>Close</button>
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}

type OpenLibraryDoc = {
  title?: string
  author_name?: string[]
  cover_i?: number
}

type OpenLibrarySearchResponse = {
  docs: OpenLibraryDoc[]
}

function pickBestCover(docs: OpenLibraryDoc[], book: Book) {
  const normalizedTitle = normalize(book.title)
  const normalizedAuthor = normalize(book.author)

  return docs
    .filter(doc => doc.cover_i)
    .sort((a, b) => scoreCoverMatch(b, normalizedTitle, normalizedAuthor) - scoreCoverMatch(a, normalizedTitle, normalizedAuthor))[0]
}

function scoreCoverMatch(doc: OpenLibraryDoc, normalizedTitle: string, normalizedAuthor: string) {
  const title = normalize(doc.title ?? '')
  const authors = (doc.author_name ?? []).map(normalize).join(' ')
  let score = 0

  if (title === normalizedTitle) score += 4
  else if (title.includes(normalizedTitle) || normalizedTitle.includes(title)) score += 2

  if (authors && normalizedAuthor) {
    if (authors.includes(normalizedAuthor)) score += 4
    else {
      const authorTokens = normalizedAuthor.split(' ').filter(token => token.length > 2)
      score += authorTokens.filter(token => authors.includes(token)).length
    }
  }

  return score
}

function normalize(value: string) {
  return value.toLowerCase().replace(/[^a-z0-9]+/g, ' ').trim()
}
