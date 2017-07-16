FROM node:8.1.4
RUN mkdir -p /app
WORKDIR /app
COPY package.json /app
RUN npm install
COPY . /app
EXPOSE 443
CMD ["node","server"]