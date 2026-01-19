# Build do Angular
FROM node:20 AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build --configuration=production

# Servidor Nginx para rodar o app
FROM nginx:alpine
# Copia o build gerado para a pasta do Nginx
# ATENÇÃO: Verifique se sua pasta dist tem subpasta 'browser'. No Angular 17/18/19 geralmente é:
COPY --from=build /app/dist/ats-frontend/browser /usr/share/nginx/html

# Copia configuração customizada do Nginx (Criaremos no próximo passo)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80