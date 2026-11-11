# ==============================================================================
# TanErp Frontend Production Dockerfile (Next.js 16 + React 19)
# Multi-stage build: deps -> builder -> lightweight runner
# ==============================================================================

# Stage 1: Dependencies installation
FROM node:22-alpine AS deps
WORKDIR /app

# Install build dependencies if needed (e.g., libc6-compat for alpine)
RUN apk add --no-cache libc6-compat

# Copy package manifests
COPY frontend/package.json frontend/package-lock.json ./

# Clean install of all dependencies
RUN npm ci

# Stage 2: Application Builder
FROM node:22-alpine AS builder
WORKDIR /app

COPY --from=deps /app/node_modules ./node_modules
COPY frontend/ ./

# Build arguments for public environment variables (baked at build-time if provided)
ARG NEXT_PUBLIC_API_BASE_URL=""
ARG NEXT_PUBLIC_APP_URL=""
ARG NEXT_PUBLIC_GOOGLE_CLIENT_ID=""
ARG NEXT_PUBLIC_MEDIA_ALLOWED_ORIGINS=""

ENV NEXT_PUBLIC_API_BASE_URL=$NEXT_PUBLIC_API_BASE_URL \
    NEXT_PUBLIC_APP_URL=$NEXT_PUBLIC_APP_URL \
    NEXT_PUBLIC_GOOGLE_CLIENT_ID=$NEXT_PUBLIC_GOOGLE_CLIENT_ID \
    NEXT_PUBLIC_MEDIA_ALLOWED_ORIGINS=$NEXT_PUBLIC_MEDIA_ALLOWED_ORIGINS \
    NEXT_TELEMETRY_DISABLED=1 \
    NODE_ENV=production

# Compile Next.js production build
RUN npm run build

# Stage 3: Production Runner
FROM node:22-alpine AS runner
WORKDIR /app

ENV NODE_ENV=production \
    PORT=3000 \
    HOSTNAME="0.0.0.0" \
    NEXT_TELEMETRY_DISABLED=1

# Create unprivileged system user and group for runtime execution
RUN addgroup --system --gid 1001 nodejs && \
    adduser --system --uid 1001 nextjs

# Copy runtime assets and dependencies
COPY --from=builder /app/package.json ./package.json
COPY --from=builder /app/package-lock.json ./package-lock.json
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/next.config.ts ./next.config.ts

# Set correct ownership for application directory
RUN chown -R nextjs:nodejs /app

USER nextjs

EXPOSE 3000

CMD ["npm", "run", "start"]
