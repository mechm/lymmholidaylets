# Install dependencies only when needed
FROM node:18-alpine AS deps
WORKDIR /src/app
COPY src/lymmholidaylets.ui/package.json src/lymmholidaylets.ui/package-lock.json* ./
RUN npm ci

# Build the Next.js app
FROM node:18-alpine AS builder
WORKDIR /src/app
COPY --from=deps /src/app/node_modules ./node_modules
COPY src/lymmholidaylets.ui/. .
ENV NEXT_BUILD_ENV=true
RUN npm run build

# Production image
FROM node:18-alpine AS runner
WORKDIR /src/app

# If you use next/image with Image Optimization, uncomment the following
# RUN apk add --no-cache libc6-compat

ENV NODE_ENV=production

COPY --from=builder /src/app/.next ./.next
COPY --from=builder /src/app/public ./public
COPY --from=builder /src/app/package.json ./package.json
COPY --from=builder /src/app/node_modules ./node_modules

EXPOSE 3000

CMD ["npm", "start"]