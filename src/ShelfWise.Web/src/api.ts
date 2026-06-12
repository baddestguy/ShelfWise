import type { Role } from './types'

export async function request<T>(path: string, options: RequestInit = {}, role: Role = 'Patron'): Promise<T> {
  const response = await fetch(path, {
    headers: { 'Content-Type': 'application/json', 'X-User-Role': role, ...(options.headers || {}) },
    ...options
  })

  if (!response.ok) {
    let message = `Request failed with ${response.status}`
    try {
      const payload = await response.json()
      message = payload.message || payload.title || message
    } catch {
      // Keep the status-based message when the response has no JSON body.
    }
    throw new Error(message)
  }

  if (response.status === 204) return null as T

  const text = await response.text()
  return text ? JSON.parse(text) as T : null as T
}
