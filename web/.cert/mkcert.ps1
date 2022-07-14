choco install mkcert
mkcert -install
cd web
mkdir .cert
mkcert -key-file ./.cert/key.pem -cert-file ./.cert/cert.pem "localhost"