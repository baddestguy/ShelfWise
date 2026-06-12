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
  if (!book) return null

  return (
    <div className="modal-backdrop" role="presentation">
      <section className="modal detail-modal" role="dialog" aria-modal="true">
        <div className="book-detail-layout">
          <div className="book-cover-placeholder" aria-hidden="true">
            <span>{book.title.slice(0, 1).toUpperCase()}</span>
          </div>

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
