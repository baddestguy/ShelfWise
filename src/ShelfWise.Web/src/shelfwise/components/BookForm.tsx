import type { BookFormState } from '../types'

type BookFormProps = {
  form: BookFormState
  editingId: number | null
  saving: boolean
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
  onChange: (field: keyof BookFormState, value: string) => void
  onCancel: () => void
}

export function BookForm({ form, editingId, saving, onSubmit, onChange, onCancel }: BookFormProps) {
  return (
    <form className="book-form" onSubmit={onSubmit}>
      <h2>{editingId ? 'Edit Book' : 'Add Book'}</h2>
      <label>
        Title
        <input value={form.title} onChange={event => onChange('title', event.target.value)} required />
      </label>
      <label>
        Author
        <input value={form.author} onChange={event => onChange('author', event.target.value)} required />
      </label>
      <div className="form-row">
        <label>
          Category
          <select value={form.category} onChange={event => onChange('category', event.target.value)}>
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
            onChange={event => onChange('totalCopies', event.target.value)}
            required
          />
        </label>
      </div>
      <label>
        Genre
        <input value={form.genre} onChange={event => onChange('genre', event.target.value)} />
      </label>
      <div className="form-actions">
        <button type="submit" disabled={saving}>{saving ? 'Saving...' : editingId ? 'Save Changes' : 'Add Book'}</button>
        {editingId && <button type="button" className="secondary" onClick={onCancel}>Cancel</button>}
      </div>
    </form>
  )
}
