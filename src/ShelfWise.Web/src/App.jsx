import React, {useEffect, useState} from 'react'

export default function App(){
  const [books, setBooks] = useState([])

  useEffect(()=>{
    fetch('/api/books')
      .then(r=>r.json())
      .then(data=>setBooks(data))
      .catch(()=>setBooks([]))
  },[])

  return (
    <div style={{padding:20}}>
      <h1>ShelfWise</h1>
      <h2>Books</h2>
      {books.length === 0 ? <p>No books yet.</p> : (
        <ul>
          {books.map(b => <li key={b.id}>{b.title} — {b.author}</li>)}
        </ul>
      )}
    </div>
  )
}
