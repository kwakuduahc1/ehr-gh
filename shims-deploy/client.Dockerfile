# Stage 1: Build Angular app
FROM node:22-alpine AS build
WORKDIR /app
COPY ../shims-client/package*.json ./
RUN npm ci
COPY ../shims-client .
RUN npm run build

# Stage 2: Serve with nginx
FROM nginx:1.27-alpine
COPY --from=build /app/dist/shims-client/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
