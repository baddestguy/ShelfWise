import { useEffect, useMemo, useState } from 'react'
import type { Book } from '../types'

const pageSizeOptions = [10, 25, 50]

type BookTableProps = {
  books: Book[]
  loading: boolean
  canManageBooks: boolean
  canDeleteBooks: boolean
  onEdit: (book: Book) => void
  onDelete: (book: Book) => void
  onOpenCirculation: (mode: 'checkout' | 'checkin', book: Book) => void
}

export function BookTable({
  books,
  loading,
  canManageBooks,
  canDeleteBooks,
  onEdit,
  onDelete,
  onOpenCirculation
}: BookTableProps) {
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const pageCount = Math.max(1, Math.ceil(books.length / pageSize))

  useEffect(() => {
    setPage(1)
  }, [books.length, pageSize])

  const pagedBooks = useMemo(() => {
    const start = (page - 1) * pageSize
    return books.slice(start, start + pageSize)
  }, [books, page, pageSize])

  if (loading) {
    return <section className="book-table-panel"><div className="empty-state">Loading books...</div></section>
  }

  if (books.length === 0) {
    return <section className="book-table-panel"><div className="empty-state">No books match the current search.</div></section>
  }

  return (
    <section className="book-table-panel">
      <div className="table-scroll">
        <table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Author</th>
              <th>Category</th>
              <th>Genre</th>
              <th>Total</th>
              <th>Out</th>
              <th>Available</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {pagedBooks.map(book => (
              <tr key={book.id}>
                <td className="title-cell">{book.title}</td>
                <td>{book.author}</td>
                <td>{book.category}</td>
                <td>{book.genre || 'Unassigned'}</td>
                <td>{book.totalCopies}</td>
                <td>{book.checkedOutCopies}</td>
                <td>
                  <span className={book.availableCopies > 0 ? 'pill available' : 'pill unavailable'}>
                    {book.availableCopies}
                  </span>
                </td>
                <td>
                  <div className="row-actions">
                    {canManageBooks && (
                      <>
                        <button type="button" className="secondary" onClick={() => onEdit(book)}>Edit</button>
                        {book.availableCopies > 0 && (
                          <button type="button" onClick={() => onOpenCirculation('checkout', book)}>
                            Check Out
                          </button>
                        )}
                        {book.checkedOutCopies > 0 && (
                          <button type="button" className="secondary" onClick={() => onOpenCirculation('checkin', book)}>
                            Check In
                          </button>
                        )}
                      </>
                    )}
                    {canDeleteBooks && <button type="button" className="danger" onClick={() => onDelete(book)}>Delete</button>}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="pagination">
        <span>
          Showing {(page - 1) * pageSize + 1}-{Math.min(page * pageSize, books.length)} of {books.length}
        </span>
        <div className="pagination-controls">
          <label>
            Rows
            <select value={pageSize} onChange={event => setPageSize(Number(event.target.value))}>
              {pageSizeOptions.map(option => <option key={option} value={option}>{option}</option>)}
            </select>
          </label>
          <button type="button" className="secondary" disabled={page === 1} onClick={() => setPage(current => Math.max(1, current - 1))}>
            Previous
          </button>
          <span>Page {page} of {pageCount}</span>
          <button type="button" className="secondary" disabled={page === pageCount} onClick={() => setPage(current => Math.min(pageCount, current + 1))}>
            Next
          </button>
        </div>
      </div>
    </section>
  )
}
