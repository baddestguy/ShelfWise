export type Role = 'Patron' | 'Librarian' | 'Admin'

export type BookFormState = {
  title: string
  author: string
  category: 'Fiction' | 'NonFiction'
  genre: string
  totalCopies: number | string
}

export type UserFormState = {
  firstName: string
  lastName: string
}

export type Book = {
  id: number
  title: string
  author: string
  category: 'Fiction' | 'NonFiction' | string
  genre: string
  totalCopies: number
  checkedOutCopies: number
  availableCopies: number
}

export type User = {
  id: number
  firstName: string
  lastName: string
}

export type AiBookMatch = {
  book: Book
  score: number
  reason: string
}

export type AiBookSearchResult = {
  query: string
  mode: string
  summary: string
  matches: AiBookMatch[]
}

export type CirculationMode = 'checkout' | 'checkin'

export type CirculationState = {
  mode: CirculationMode
  book: Book
}
