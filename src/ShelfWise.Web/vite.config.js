import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const allowedHosts = (process.env.VITE_ALLOWED_HOSTS || 'shelfwise-web-production.up.railway.app')
  .split(',')
  .map(host => host.trim())
  .filter(Boolean)
const apiTarget = process.env.VITE_API_TARGET || 'http://api:80'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    // bind to all interfaces so Vite is reachable from the host when running inside Docker
    host: '0.0.0.0',
    allowedHosts,
    // when running in Docker Compose the API is available at the `api` service
    // and listens on port 80 inside the container
    proxy: {
      '/api': apiTarget
    }
  }
})
