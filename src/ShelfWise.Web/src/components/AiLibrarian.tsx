import type { AiBookSearchResult } from '../types'

type AiLibrarianProps = {
  query: string
  result: AiBookSearchResult | null
  searching: boolean
  onQueryChange: (query: string) => void
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
}

export function AiLibrarian({ query, result, searching, onQueryChange, onSubmit }: AiLibrarianProps) {
  return (
    <form className="ai-form" onSubmit={onSubmit}>
      <h2>AI Librarian</h2>
      <label>
        Ask for a recommendation
        <textarea
          value={query}
          onChange={event => onQueryChange(event.target.value)}
          placeholder="I want practical books about writing better software"
          required
        />
      </label>
      <button type="submit" disabled={searching || query.trim().length === 0}>
        {searching ? 'Searching...' : 'Find Matches'}
      </button>
      {result && (
        <div className="ai-results">
          <p className="ai-mode">{result.mode}</p>
          <p>{result.summary}</p>
          <ol>
            {result.matches.map(match => (
              <li key={match.book.id}>
                <strong>{match.book.title}</strong>
                <span>{match.book.author} · Score {match.score}</span>
                <p>{match.reason}</p>
              </li>
            ))}
          </ol>
        </div>
      )}
    </form>
  )
}
