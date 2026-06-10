import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    // bind to all interfaces so Vite is reachable from the host when running inside Docker
    host: '0.0.0.0',
    // when running in Docker Compose the API is available at the `api` service
    // and listens on port 80 inside the container
    proxy: {
      '/api': 'http://api:80'
    }
  }
})
