# Verwende ein Basis-Image mit einem Webserver (z. B. nginx)
FROM nginx:latest

# Kopiere die index.html-Datei in das Standardverzeichnis des Websevers
COPY ./web /usr/share/nginx/html/

# Setze den Port, auf dem der Webserver lauschen soll
EXPOSE 80
EXPOSE 443