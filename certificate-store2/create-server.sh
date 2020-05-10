ROOT_PASSWORD=privat3

NAME=server
CERT_PASSWORD=$NAME

openssl genrsa -aes256 -passout pass:$CERT_PASSWORD -out intermediate/private/$NAME.key.pem 2048
chmod 400 intermediate/private/$NAME.key.pem

openssl req -config root.cnf \
      -passin pass:$CERT_PASSWORD -key intermediate/private/$NAME.key.pem \
      -new -sha256 \
	  -subj "//C=RO\ST=Iasi\O=Security Lab\OU=IT\CN=$1" \
	  -out intermediate/csr/$NAME.csr.pem
	  
openssl ca -config root.cnf \
      -extensions server_cert -days 375 -notext -md sha256 \
	  -passin pass:$ROOT_PASSWORD \
      -in intermediate/csr/$NAME.csr.pem \
      -out intermediate/certs/$NAME.cert.pem
chmod 444 intermediate/certs/$NAME.cert.pem

winpty openssl pkcs12 -export \
 -inkey intermediate/private/$NAME.key.pem -passin pass:$CERT_PASSWORD \
 -in intermediate/certs/$NAME.cert.pem -certfile certs/ca.cert.pem \
 -passout pass:$CERT_PASSWORD -out intermediate/certs/$NAME.pfx