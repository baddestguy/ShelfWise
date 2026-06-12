FROM node:20-alpine
WORKDIR /app
# copy package files first to leverage Docker layer caching
COPY package.json package-lock.json* ./
# Rollup/Vite use platform-specific optional dependencies. Force npm to install
# them inside the Linux container instead of relying on the host-generated tree.
RUN sh -c "if [ -f package-lock.json ]; then npm ci --include=optional --silent; else npm install --include=optional --silent; fi"
COPY . .
EXPOSE 3000
CMD ["npm", "run", "dev"]
