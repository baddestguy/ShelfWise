const { expect, test } = require('@playwright/test')

test('loads the catalog from the real backend', async ({ page }) => {
  await page.goto('/')

  await expect(page.getByRole('heading', { name: 'ShelfWise' })).toBeVisible()
  await expect(page.getByText(/books in view/i)).toBeVisible()
  await expect(page.getByRole('cell', { name: '1984' })).toBeVisible()
})

test('search filters the catalog by title', async ({ page }) => {
  await page.goto('/')

  await page.getByPlaceholder('Title, author, genre, category').fill('dune')

  await expect(page.getByRole('cell', { name: 'Dune' })).toBeVisible()
  await expect(page.getByRole('cell', { name: '1984' })).toHaveCount(0)
})

test('login reveals role-based management controls', async ({ page }) => {
  await page.goto('/')

  await expect(page.getByRole('button', { name: 'Add Book' })).toHaveCount(0)

  await page.getByLabel('Username').fill('librarian@shelfwise.dev')
  await page.getByLabel('Password').fill('Password123!')
  await page.getByRole('button', { name: 'Sign In' }).click()

  await expect(page.locator('.session-card strong')).toHaveText('Librarian')
  await expect(page.getByRole('button', { name: 'Add Book' })).toBeVisible()
})
