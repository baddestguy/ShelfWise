import React, { useEffect, useMemo, useState } from 'react'
import './styles.css'

const emptyBook = {
  title: '',
  author: '',
  category: 'NonFiction',
  genre: '',
  totalCopies: 1
}

const emptyUser = {
  firstName: '',
  lastName: ''
}

async function request(path, options = {}, role = 'Patron') {
  const response = await fetch(path, {
    headers: { 'Content-Type': 'application/json', 'X-User-Role': role, ...(options.headers || {}) },
    ...options
  })

  if (!response.ok) {
    let message = `Request failed with ${response.status}`
    try {
      const payload = await response.json()
      message = payload.message || payload.title || message
    } catch {
      // Keep the status-based message when the response has no JSON body.
    }
    throw new Error(message)
  }

  if (response.status === 204) return null

  const text = await response.text()
  return text ? JSON.parse(text) : null
}

export default function App() {
  const [books, setBooks] = useState([])
  const [users, setUsers] = useState([])
  const [role, setRole] = useState('Librarian')
  const [search, setSearch] = useState('')
  const [circulation, setCirculation] = useState(null)
  const [circulationUserId, setCirculationUserId] = useState('')
  const [form, setForm] = useState(emptyBook)
  const [userForm, setUserForm] = useState(emptyUser)
  const [editingId, setEditingId] = useState(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [savingUser, setSavingUser] = useState(false)
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [modalError, setModalError] = useState('')

  const circulationUserName = useMemo(() => {
    const selected = users.find(user => String(user.id) === String(circulationUserId))
    return selected ? `${selected.firstName} ${selected.lastName}` : ''
  }, [circulationUserId, users])
  const canManageBooks = role === 'Librarian' || role === 'Admin'
  const canDeleteBooks = role === 'Admin'
  const canCreateUsers = role === 'Admin'

  async function loadBooks(nextSearch = search) {
    const query = nextSearch.trim()
    const data = await request(`/api/books${query ? `?search=${encodeURIComponent(query)}` : ''}`, {}, role)
    setBooks(data)
  }

  async function loadInitialData() {
    setLoading(true)
    setError('')
    try {
      const [bookData, userData] = await Promise.all([
        request('/api/books', {}, role),
        request('/api/users', {}, role)
      ])
      setBooks(bookData)
      setUsers(userData)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadInitialData()
  }, [role])

  useEffect(() => {
    const handle = window.setTimeout(async () => {
      try {
        setError('')
        await loadBooks(search)
      } catch (err) {
        setError(err.message)
      }
    }, 250)

    return () => window.clearTimeout(handle)
  }, [search])

  function updateForm(field, value) {
    setForm(current => ({ ...current, [field]: value }))
  }

  function updateUserForm(field, value) {
    setUserForm(current => ({ ...current, [field]: value }))
  }

  function resetForm() {
    setForm(emptyBook)
    setEditingId(null)
  }

  function editBook(book) {
    setEditingId(book.id)
    setForm({
      title: book.title,
      author: book.author,
      category: book.category,
      genre: book.genre,
      totalCopies: book.totalCopies
    })
  }

  async function saveBook(event) {
    event.preventDefault()
    setSaving(true)
    setError('')
    setMessage('')

    const payload = {
      ...form,
      title: form.title.trim(),
      author: form.author.trim(),
      genre: form.genre.trim(),
      totalCopies: Number(form.totalCopies)
    }

    try {
      if (editingId) {
        await request(`/api/books/${editingId}`, {
          method: 'PATCH',
          body: JSON.stringify(payload)
        }, role)
        setMessage('Book updated.')
      } else {
        await request('/api/books', {
          method: 'POST',
          body: JSON.stringify(payload)
        }, role)
        setMessage('Book added.')
      }
      resetForm()
      await loadBooks()
    } catch (err) {
      setError(err.message)
    } finally {
      setSaving(false)
    }
  }

  async function createUser(event) {
    event.preventDefault()
    setSavingUser(true)
    setError('')
    setMessage('')

    const payload = {
      firstName: userForm.firstName.trim(),
      lastName: userForm.lastName.trim()
    }

    try {
      const created = await request('/api/users', {
        method: 'POST',
        body: JSON.stringify(payload)
      }, role)
      const nextUsers = await request('/api/users', {}, role)
      setUsers(nextUsers)
      setUserForm(emptyUser)
      setMessage(`User created: ${created.firstName} ${created.lastName}.`)
    } catch (err) {
      setError(err.message)
    } finally {
      setSavingUser(false)
    }
  }

  async function deleteBook(book) {
    if (!window.confirm(`Delete "${book.title}"?`)) return
    setError('')
    setMessage('')

    try {
      await request(`/api/books/${book.id}`, { method: 'DELETE' }, role)
      setMessage('Book deleted.')
      await loadBooks()
    } catch (err) {
      setError(err.message)
    }
  }

  function openCirculationModal(mode, book) {
    setError('')
    setMessage('')
    setModalError('')
    setCirculation({ mode, book })
    setCirculationUserId(users.length > 0 ? String(users[0].id) : '')
  }

  function closeCirculationModal() {
    setCirculation(null)
    setCirculationUserId('')
    setModalError('')
  }

  async function submitCirculation(event) {
    event.preventDefault()
    if (!circulation) return
    if (!circulationUserId) {
      setModalError('Select a user first.')
      return
    }

    setError('')
    setMessage('')
    setModalError('')

    try {
      const action = circulation.mode === 'checkout' ? 'checkout' : 'checkin'
      const payload = circulation.mode === 'checkout'
        ? { userId: Number(circulationUserId), dueDays: 14 }
        : { userId: Number(circulationUserId) }

      await request(`/api/books/${circulation.book.id}/${action}`, {
        method: 'POST',
        body: JSON.stringify(payload)
      }, role)
      const verb = circulation.mode === 'checkout' ? 'Checked out' : 'Checked in'
      const suffix = circulation.mode === 'checkout' ? ` to ${circulationUserName}` : ` from ${circulationUserName}`
      setMessage(`${verb} "${circulation.book.title}"${suffix}.`)
      closeCirculationModal()
      await loadBooks()
    } catch (err) {
      setModalError(err.message)
    }
  }

  return (
    <main className="app-shell">
      <header className="topbar">
        <div>
          <h1>ShelfWise</h1>
          <p>{books.length} books in view</p>
        </div>
        <div className="header-controls">
          <label className="role-picker">
            Role
            <select value={role} onChange={event => setRole(event.target.value)}>
              <option value="Patron">Patron</option>
              <option value="Librarian">Librarian</option>
              <option value="Admin">Admin</option>
            </select>
          </label>
        </div>
      </header>

      <section className="toolbar">
        <label className="search-field">
          Search
          <input
            value={search}
            onChange={event => setSearch(event.target.value)}
            placeholder="Title, author, genre, category"
          />
        </label>
      </section>

      {(message || error) && (
        <section className={`notice ${error ? 'error' : 'success'}`}>
          {error || message}
        </section>
      )}

      <section className="workspace">
        <aside className="side-panel">
          {canManageBooks ? (
            <form className="book-form" onSubmit={saveBook}>
              <h2>{editingId ? 'Edit Book' : 'Add Book'}</h2>
              <label>
                Title
                <input value={form.title} onChange={event => updateForm('title', event.target.value)} required />
              </label>
              <label>
                Author
                <input value={form.author} onChange={event => updateForm('author', event.target.value)} required />
              </label>
              <div className="form-row">
                <label>
                  Category
                  <select value={form.category} onChange={event => updateForm('category', event.target.value)}>
                    <option value="NonFiction">NonFiction</option>
                    <option value="Fiction">Fiction</option>
                  </select>
                </label>
                <label>
                  Copies
                  <input
                    type="number"
                    min="0"
                    value={form.totalCopies}
                    onChange={event => updateForm('totalCopies', event.target.value)}
                    required
                  />
                </label>
              </div>
              <label>
                Genre
                <input value={form.genre} onChange={event => updateForm('genre', event.target.value)} />
              </label>
              <div className="form-actions">
                <button type="submit" disabled={saving}>{saving ? 'Saving...' : editingId ? 'Save Changes' : 'Add Book'}</button>
                {editingId && <button type="button" className="secondary" onClick={resetForm}>Cancel</button>}
              </div>
            </form>
          ) : (
            <section className="permission-card">
              <h2>Read Only</h2>
              <p>Patrons can search and view availability. Switch to Librarian or Admin to manage circulation.</p>
            </section>
          )}

          {canCreateUsers && (
            <form className="user-form" onSubmit={createUser}>
              <h2>Add User</h2>
              <label>
                First name
                <input value={userForm.firstName} onChange={event => updateUserForm('firstName', event.target.value)} required />
              </label>
              <label>
                Last name
                <input value={userForm.lastName} onChange={event => updateUserForm('lastName', event.target.value)} required />
              </label>
              <button type="submit" disabled={savingUser}>
                {savingUser ? 'Creating...' : 'Create User'}
              </button>
            </form>
          )}
        </aside>

        <section className="book-table-panel">
          {loading ? (
            <div className="empty-state">Loading books...</div>
          ) : books.length === 0 ? (
            <div className="empty-state">No books match the current search.</div>
          ) : (
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
                {books.map(book => (
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
                            <button type="button" className="secondary" onClick={() => editBook(book)}>Edit</button>
                            {book.availableCopies > 0 && (
                              <button type="button" onClick={() => openCirculationModal('checkout', book)}>
                                Check Out
                              </button>
                            )}
                            {book.checkedOutCopies > 0 && (
                              <button type="button" className="secondary" onClick={() => openCirculationModal('checkin', book)}>
                                Check In
                              </button>
                            )}
                          </>
                        )}
                        {canDeleteBooks && <button type="button" className="danger" onClick={() => deleteBook(book)}>Delete</button>}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </section>
      </section>

      {circulation && (
        <div className="modal-backdrop" role="presentation">
          <form className="modal" onSubmit={submitCirculation}>
            <div>
              <h2>{circulation.mode === 'checkout' ? 'Check Out Book' : 'Check In Book'}</h2>
              <p>{circulation.book.title}</p>
            </div>
            <label>
              User
              <select value={circulationUserId} onChange={event => setCirculationUserId(event.target.value)} required>
                {users.length === 0 && <option value="">No users available</option>}
                {users.map(user => (
                  <option key={user.id} value={user.id}>
                    {user.firstName} {user.lastName}
                  </option>
                ))}
              </select>
            </label>
            {modalError && <div className="modal-error">{modalError}</div>}
            <div className="modal-actions">
              <button type="submit" disabled={users.length === 0}>
                {circulation.mode === 'checkout' ? 'Check Out' : 'Check In'}
              </button>
              <button type="button" className="secondary" onClick={closeCirculationModal}>
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}
    </main>
  )
}
