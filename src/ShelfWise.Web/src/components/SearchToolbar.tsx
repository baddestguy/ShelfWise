type SearchToolbarProps = {
  search: string
  onSearchChange: (search: string) => void
}

export function SearchToolbar({ search, onSearchChange }: SearchToolbarProps) {
  return (
    <section className="toolbar">
      <label className="search-field">
        Search
        <input
          value={search}
          onChange={event => onSearchChange(event.target.value)}
          placeholder="Title, author, genre, category"
        />
      </label>
    </section>
  )
}
