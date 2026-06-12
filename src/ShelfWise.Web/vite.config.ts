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
    host: '0.0.0.0',
    allowedHosts,
    proxy: {
      '/api': apiTarget
    }
  }
})
