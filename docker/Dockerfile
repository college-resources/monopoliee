FROM node:12

WORKDIR /app
COPY ./package* /app/
RUN npm ci

COPY . /app

ENV NODE_ENV=production
ENTRYPOINT [ "node", "." ]
