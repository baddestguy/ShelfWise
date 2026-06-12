import type { UserFormState } from '../types'

type UserFormProps = {
  form: UserFormState
  saving: boolean
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
  onChange: (field: keyof UserFormState, value: string) => void
}

export function UserForm({ form, saving, onSubmit, onChange }: UserFormProps) {
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
      <button type="submit" disabled={saving}>
        {saving ? 'Creating...' : 'Create User'}
      </button>
    </form>
  )
}
