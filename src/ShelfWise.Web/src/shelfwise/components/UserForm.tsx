import type { UserFormState } from '../types'

type UserFormProps = {
  form: UserFormState
  saving: boolean
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
  onChange: (field: keyof UserFormState, value: string) => void
  onCancel: () => void
}

export function UserForm({ form, saving, onSubmit, onChange, onCancel }: UserFormProps) {
  return (
    <form className="user-form" onSubmit={onSubmit}>
      <h2>Add User</h2>
      <label>
        First name
        <input value={form.firstName} onChange={event => onChange('firstName', event.target.value)} required />
      </label>
      <label>
        Last name
        <input value={form.lastName} onChange={event => onChange('lastName', event.target.value)} required />
      </label>
      <div className="form-actions">
        <button type="submit" disabled={saving}>
          {saving ? 'Creating...' : 'Create User'}
        </button>
        <button type="button" className="secondary" onClick={onCancel}>Cancel</button>
      </div>
    </form>
  )
}
