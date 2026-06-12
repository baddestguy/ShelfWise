import type { Role } from '../types'

type HeaderProps = {
  bookCount: number
  role: Role
  onRoleChange: (role: Role) => void
}

export function Header({ bookCount, role, onRoleChange }: HeaderProps) {
  return (
    <header className="topbar">
      <div>
        <h1>ShelfWise</h1>
        <p>{bookCount} books in view</p>
      </div>
      <div className="header-controls">
        <label className="role-picker">
          Role
          <select value={role} onChange={event => onRoleChange(event.target.value as Role)}>
            <option value="Patron">Patron</option>
            <option value="Librarian">Librarian</option>
            <option value="Admin">Admin</option>
          </select>
        </label>
      </div>
    </header>
  )
}
