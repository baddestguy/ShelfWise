import type { AuthUser } from '../types'

type HeaderProps = {
  bookCount: number
  user: AuthUser | null
  username: string
  password: string
  loggingIn: boolean
  onUsernameChange: (value: string) => void
  onPasswordChange: (value: string) => void
  onLogin: (event: React.FormEvent<HTMLFormElement>) => void
  onLogout: () => void
}

export function Header({
  bookCount,
  user,
  username,
  password,
  loggingIn,
  onUsernameChange,
  onPasswordChange,
  onLogin,
  onLogout
}: HeaderProps) {
  return (
    <header className="topbar">
      <div>
        <h1>ShelfWise</h1>
        <p>{bookCount} books in view</p>
      </div>
      <div className="header-controls">
        {user ? (
          <div className="session-card">
            <strong>{user.role}</strong>
            <button type="button" className="secondary" onClick={onLogout}>Log Out</button>
          </div>
        ) : (
          <form className="login-form" onSubmit={onLogin}>
            <label>
              Username
              <input value={username} onChange={event => onUsernameChange(event.target.value)} placeholder="patron@shelfwise.dev" required />
            </label>
            <label>
              Password
              <input type="password" value={password} onChange={event => onPasswordChange(event.target.value)} placeholder="Password123!" required />
            </label>
            <button type="submit" disabled={loggingIn}>{loggingIn ? 'Signing In...' : 'Sign In'}</button>
          </form>
        )}
      </div>
    </header>
  )
}
