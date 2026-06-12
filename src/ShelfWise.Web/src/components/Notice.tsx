type NoticeProps = {
  message: string
  error: string
}

export function Notice({ message, error }: NoticeProps) {
  if (!message && !error) return null

  return (
    <section className={`notice ${error ? 'error' : 'success'}`}>
      {error || message}
    </section>
  )
}
