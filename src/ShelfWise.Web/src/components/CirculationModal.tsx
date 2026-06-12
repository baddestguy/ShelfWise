import type { CirculationState, User } from '../types'

type CirculationModalProps = {
  circulation: CirculationState | null
  users: User[]
  selectedUserId: string
  error: string
  onSelectedUserChange: (userId: string) => void
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
  onClose: () => void
}

export function CirculationModal({
  circulation,
  users,
  selectedUserId,
  error,
  onSelectedUserChange,
  onSubmit,
  onClose
}: CirculationModalProps) {
  if (!circulation) return null

  return (
    <div className="modal-backdrop" role="presentation">
      <form className="modal" onSubmit={onSubmit}>
        <div>
          <h2>{circulation.mode === 'checkout' ? 'Check Out Book' : 'Check In Book'}</h2>
          <p>{circulation.book.title}</p>
        </div>
        <label>
          User
          <select value={selectedUserId} onChange={event => onSelectedUserChange(event.target.value)} required>
            {users.length === 0 && <option value="">No users available</option>}
            {users.map(user => (
              <option key={user.id} value={user.id}>
                {user.firstName} {user.lastName}
              </option>
            ))}
          </select>
        </label>
        {error && <div className="modal-error">{error}</div>}
        <div className="modal-actions">
          <button type="submit" disabled={users.length === 0}>
            {circulation.mode === 'checkout' ? 'Check Out' : 'Check In'}
          </button>
          <button type="button" className="secondary" onClick={onClose}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}
