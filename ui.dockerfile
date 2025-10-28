# Multi-stage build for Angular application
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Copy package files from ui directory
COPY ui/package*.json ./

# Install dependencies including Angular CLI
RUN npm ci

# Copy source code from ui directory
COPY ui/ .

# Build the application
RUN npm run build

# Production stage with Nginx
FROM nginx:alpine

# Copy built application from build stage
COPY --from=build /app/dist/task-manager-ui/browser /usr/share/nginx/html

# Copy custom nginx configuration from ui directory
COPY ui/nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
