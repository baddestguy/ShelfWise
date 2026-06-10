FROM node:20-alpine
WORKDIR /app
# copy package files first to leverage Docker layer caching
COPY package.json package-lock.json* ./
# prefer `npm ci` for reproducible installs, but fall back to `npm install`
# when package-lock.json is not present (avoids build failure in scaffold)
RUN sh -c "if [ -f package-lock.json ]; then npm ci --silent; else npm install --silent; fi"
COPY . .
EXPOSE 3000
CMD ["npm", "run", "dev"]
