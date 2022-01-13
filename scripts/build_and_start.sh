cd ../
docker build -t aspblazorapp6 .
docker run -d -p 8080:80 --name gonis-assistant aspblazorapp6
open http://localhost:8080