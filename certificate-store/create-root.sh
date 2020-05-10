PASSWORD=privat3

chmod 700 private
touch index.txt
echo 1000 > serial

winpty openssl genrsa -aes256 -passout pass:$ -out private/ca.key.pem 4096
chmod 400 private/ca.key.pem

winpty openssl req -config ./root.cnf -passin pass:$PASSWORD -key private/ca.key.pem -new -x509 -days 7300 -sha256 -extensions v3_ca \
	-subj "//C=RO\ST=Iasi\O=Security Lab\OU=IT\CN=Root Certificate" -out certs/ca.cert.pem
chmod 444 certs/ca.cert.pem

