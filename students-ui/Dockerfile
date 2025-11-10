# # # ========================
# # # Angular Frontend Dockerfile
# # # ========================
# # # Stage 1: Build Angular app
# # FROM node:20.19.0 AS build
# # WORKDIR /app
# # COPY package*.json ./
# # RUN npm install
# # COPY . .
# # RUN npm run build --prod

# # # Stage 2: Serve via NGINX
# # FROM nginx:1.27-alpine
# # COPY --from=build /app/dist/students-ui /usr/share/nginx/html
# # COPY nginx.conf /etc/nginx/conf.d/default.conf
# # EXPOSE 80


# # ========================
# # Stage 1: Build Angular app
# # ========================
# FROM node:20.19.0 AS build
# WORKDIR /app
# COPY package*.json ./
# RUN npm install
# COPY . .
# RUN npm run build -- --configuration production


# # ========================
# # Stage 2: Serve with Nginx
# # ========================
# FROM nginx:1.27-alpine
# COPY --from=build /app/dist/students-ui /usr/share/nginx/html
# COPY nginx.conf /etc/nginx/conf.d/default.conf
# EXPOSE 80



## works even without backend 

# ---- Build Angular ----
FROM node:20.19.0 AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
# Angular 17/18 production build
RUN npm run build -- --configuration production

# ---- Serve with Nginx ----
FROM nginx:1.27-alpine
# ⬇️ THIS PATH MUST MATCH YOUR FOLDER: dist/students-ui/browser
COPY --from=build /app/dist/students-ui/browser /usr/share/nginx/html
# Copy our Nginx site config as the default vhost
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
