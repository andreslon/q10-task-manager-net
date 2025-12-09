# Multi-stage build for Angular application
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Copy package files from ui directory
COPY ui/package*.json ./

# Install dependencies including Angular CLI
RUN npm install

# Copy source code from ui directory
COPY ui/ .

# Build the application for production
RUN npm run build --configuration=production

# Production stage with Nginx
FROM nginx:alpine

# Copy built application from build stage
COPY --from=build /app/dist/task-manager-ui /usr/share/nginx/html

# No need to rename index files for this setup

# Copy custom nginx configuration from ui directory
COPY ui/nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
