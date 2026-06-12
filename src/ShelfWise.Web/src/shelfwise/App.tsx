import { useEffect, useMemo, useState } from 'react'
import { request } from './api'
import { AiLibrarian } from './components/AiLibrarian'
import { BookDetailsModal } from './components/BookDetailsModal'
import { BookForm } from './components/BookForm'
import { BookTable } from './components/BookTable'
import { CirculationModal } from './components/CirculationModal'
import { Header } from './components/Header'
import { Notice } from './components/Notice'
import { PermissionCard } from './components/PermissionCard'
import { SearchToolbar } from './components/SearchToolbar'
import { UserForm } from './components/UserForm'
import type { AiBookSearchResult, Book, BookFormState, CirculationMode, CirculationState, Role, User, UserFormState } from './types'
import './styles.css'

const emptyBook: BookFormState = {
  title: '',
  author: '',
  category: 'NonFiction',
  genre: '',
  totalCopies: 1
}

const emptyUser: UserFormState = {
  firstName: '',
  lastName: ''
}

export default function App() {
  const [books, setBooks] = useState<Book[]>([])
  const [users, setUsers] = useState<User[]>([])
  const [role, setRole] = useState<Role>('Patron')
  const [search, setSearch] = useState('')
  const [aiQuery, setAiQuery] = useState('')
  const [aiResult, setAiResult] = useState<AiBookSearchResult | null>(null)
  const [circulation, setCirculation] = useState<CirculationState | null>(null)
  const [circulationUserId, setCirculationUserId] = useState('')
  const [selectedBook, setSelectedBook] = useState<Book | null>(null)
  const [form, setForm] = useState<BookFormState>(emptyBook)
  const [userForm, setUserForm] = useState<UserFormState>(emptyUser)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [returnToDetailsId, setReturnToDetailsId] = useState<number | null>(null)
  const [bookModalOpen, setBookModalOpen] = useState(false)
  const [userModalOpen, setUserModalOpen] = useState(false)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [aiSearching, setAiSearching] = useState(false)
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
    const data = await request<Book[]>(`/api/books${query ? `?search=${encodeURIComponent(query)}` : ''}`, {}, role)
    setBooks(data)
  }

  async function loadInitialData() {
    setLoading(true)
    setError('')
    try {
      const bookData = await request<Book[]>('/api/books', {}, role)
      const userData = canManageBooks
        ? await request<User[]>('/api/users', {}, role)
        : []

      setBooks(bookData)
      setUsers(userData)
    } catch (err) {
      setError(toErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadInitialData()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [role])

  useEffect(() => {
    if (!canManageBooks) resetForm()
    if (!canCreateUsers) closeUserModal()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [canManageBooks, canCreateUsers])

  useEffect(() => {
    const handle = window.setTimeout(async () => {
      try {
        setError('')
        await loadBooks(search)
      } catch (err) {
        setError(toErrorMessage(err))
      }
    }, 250)

    return () => window.clearTimeout(handle)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search])

  function updateForm(field: keyof BookFormState, value: string) {
    setForm(current => ({ ...current, [field]: value }))
  }

  function updateUserForm(field: keyof UserFormState, value: string) {
    setUserForm(current => ({ ...current, [field]: value }))
  }

  function resetForm() {
    const detailsId = returnToDetailsId
    setForm(emptyBook)
    setEditingId(null)
    setReturnToDetailsId(null)
    setBookModalOpen(false)

    if (detailsId) {
      const latestBook = books.find(book => book.id === detailsId)
      if (latestBook) setSelectedBook(latestBook)
    }
  }

  function openAddBookModal() {
    setError('')
    setMessage('')
    setForm(emptyBook)
    setEditingId(null)
    setReturnToDetailsId(null)
    setBookModalOpen(true)
  }

  function editBook(book: Book) {
    setError('')
    setMessage('')
    setReturnToDetailsId(selectedBook?.id === book.id ? book.id : null)
    setSelectedBook(null)
    setEditingId(book.id)
    setForm({
      title: book.title,
      author: book.author,
      category: book.category === 'Fiction' ? 'Fiction' : 'NonFiction',
      genre: book.genre,
      totalCopies: book.totalCopies
    })
    setBookModalOpen(true)
  }

  function closeUserModal() {
    setUserForm(emptyUser)
    setUserModalOpen(false)
  }

  function openUserModal() {
    setError('')
    setMessage('')
    setUserForm(emptyUser)
    setUserModalOpen(true)
  }

  async function saveBook(event: React.FormEvent<HTMLFormElement>) {
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
        await request<null>(`/api/books/${editingId}`, {
          method: 'PATCH',
          body: JSON.stringify(payload)
        }, role)
        setMessage('Book updated.')
      } else {
        await request<Book>('/api/books', {
          method: 'POST',
          body: JSON.stringify(payload)
        }, role)
        setMessage('Book added.')
      }
      const detailsId = returnToDetailsId
      setForm(emptyBook)
      setEditingId(null)
      setReturnToDetailsId(null)
      setBookModalOpen(false)
      await loadBooks()
      if (detailsId) {
        const refreshed = await request<Book>(`/api/books/${detailsId}`, {}, role)
        setSelectedBook(refreshed)
      }
    } catch (err) {
      setError(toErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  async function createUser(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSavingUser(true)
    setError('')
    setMessage('')

    const payload = {
      firstName: userForm.firstName.trim(),
      lastName: userForm.lastName.trim()
    }

    try {
      const created = await request<User>('/api/users', {
        method: 'POST',
        body: JSON.stringify(payload)
      }, role)
      const nextUsers = await request<User[]>('/api/users', {}, role)
      setUsers(nextUsers)
      closeUserModal()
      setMessage(`User created: ${created.firstName} ${created.lastName}.`)
    } catch (err) {
      setError(toErrorMessage(err))
    } finally {
      setSavingUser(false)
    }
  }

  async function runAiSearch(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setAiSearching(true)
    setError('')
    setMessage('')

    try {
      const result = await request<AiBookSearchResult>('/api/ai/book-search', {
        method: 'POST',
        body: JSON.stringify({ query: aiQuery.trim() })
      }, role)
      setAiResult(result)
    } catch (err) {
      setError(toErrorMessage(err))
    } finally {
      setAiSearching(false)
    }
  }

  async function deleteBook(book: Book) {
    if (typeof window !== 'undefined' && !window.confirm(`Delete "${book.title}"?`)) return
    setError('')
    setMessage('')
    setSelectedBook(null)

    try {
      await request<null>(`/api/books/${book.id}`, { method: 'DELETE' }, role)
      setMessage('Book deleted.')
      await loadBooks()
    } catch (err) {
      setError(toErrorMessage(err))
    }
  }

  function openCirculationModal(mode: CirculationMode, book: Book) {
    setError('')
    setMessage('')
    setModalError('')
    setSelectedBook(null)
    setCirculation({ mode, book })
    setCirculationUserId(users.length > 0 ? String(users[0].id) : '')
  }

  function closeCirculationModal() {
    setCirculation(null)
    setCirculationUserId('')
    setModalError('')
  }

  async function submitCirculation(event: React.FormEvent<HTMLFormElement>) {
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

      await request<null>(`/api/books/${circulation.book.id}/${action}`, {
        method: 'POST',
        body: JSON.stringify(payload)
      }, role)
      const verb = circulation.mode === 'checkout' ? 'Checked out' : 'Checked in'
      const suffix = circulation.mode === 'checkout' ? ` to ${circulationUserName}` : ` from ${circulationUserName}`
      setMessage(`${verb} "${circulation.book.title}"${suffix}.`)
      closeCirculationModal()
      await loadBooks()
    } catch (err) {
      setModalError(toErrorMessage(err))
    }
  }

  return (
    <main className="app-shell">
      <Header bookCount={books.length} role={role} onRoleChange={setRole} />
      <SearchToolbar search={search} onSearchChange={setSearch} />
      {(canManageBooks || canCreateUsers) && (
        <section className="action-bar" aria-label="Management actions">
          {canManageBooks && <button type="button" onClick={openAddBookModal}>Add Book</button>}
          {canCreateUsers && <button type="button" className="secondary" onClick={openUserModal}>Add User</button>}
        </section>
      )}
      <Notice message={message} error={error} />

      <section className="workspace">
        <aside className="side-panel">
          <AiLibrarian
            query={aiQuery}
            result={aiResult}
            searching={aiSearching}
            onQueryChange={setAiQuery}
            onSubmit={runAiSearch}
          />

          {!canManageBooks && <PermissionCard />}
        </aside>

        <BookTable
          books={books}
          loading={loading}
          onSelect={setSelectedBook}
        />
      </section>

      <BookDetailsModal
        book={selectedBook}
        canManageBooks={canManageBooks}
        canDeleteBooks={canDeleteBooks}
        onEdit={editBook}
        onDelete={deleteBook}
        onOpenCirculation={openCirculationModal}
        onClose={() => setSelectedBook(null)}
      />

      <CirculationModal
        circulation={circulation}
        users={users}
        selectedUserId={circulationUserId}
        error={modalError}
        onSelectedUserChange={setCirculationUserId}
        onSubmit={submitCirculation}
        onClose={closeCirculationModal}
      />

      {bookModalOpen && (
        <div className="modal-backdrop" role="presentation">
          <div className="modal form-modal" role="dialog" aria-modal="true">
            <BookForm
              form={form}
              editingId={editingId}
              saving={saving}
              onSubmit={saveBook}
              onChange={updateForm}
              onCancel={resetForm}
            />
          </div>
        </div>
      )}

      {userModalOpen && (
        <div className="modal-backdrop" role="presentation">
          <div className="modal form-modal" role="dialog" aria-modal="true">
            <UserForm
              form={userForm}
              saving={savingUser}
              onSubmit={createUser}
              onChange={updateUserForm}
              onCancel={closeUserModal}
            />
          </div>
        </div>
      )}
    </main>
  )
}

function toErrorMessage(error: unknown) {
  return error instanceof Error ? error.message : 'Something went wrong.'
}
