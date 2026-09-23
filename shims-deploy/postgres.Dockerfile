FROM postgres:18-alpine3.24
# COPY ./restore.sh /docker-entrypoint-initdb.d/restore.sh
COPY ./db-script.sql /docker-entrypoint-initdb.d/001.sql
# COPY ./server/sql/functions.sql /docker-entrypoint-initdb.d/002.sql
RUN apk update && apk upgrade && rm -rf /var/cache/apk/*
# RUN chmod +x /docker-entrypoint-initdb.d/restore.sh
ENV POSTGRES_USER=postgres
ENV POSTGRES_DB=shims
