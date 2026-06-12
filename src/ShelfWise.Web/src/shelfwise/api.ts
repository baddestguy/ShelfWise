const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.replace(/\/$/, '') ?? ''

export async function request<T>(path: string, options: RequestInit = {}, token?: string): Promise<T> {
  const headers = new Headers(options.headers)
  headers.set('Content-Type', 'application/json')
  if (token) headers.set('Authorization', `Bearer ${token}`)

  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers
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
