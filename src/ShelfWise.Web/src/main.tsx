import React from 'react'
import ReactDOM from 'react-dom/client'
import ShelfWiseApp from './shelfwise/App'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <div className="shelfwise-app">
      <ShelfWiseApp />
    </div>
  </React.StrictMode>
)
