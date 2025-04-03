
# Gerar senha de admin para o Seq ( SEQ_FIRSTRUN_ADMINPASSWORDHASH )
```bash
echo 'p@ssw0rd' | docker run --rm -i datalust/seq config hash
```

# Gerar SecretKey para o Seq ( SEQ_STORAGE_SECRETKEY )
```bash
docker run --rm -it datalust/seq show-key --generate
```

# Executando o docker compose
```bash
COMPOSE_BAKE=true docker compose up -d
```

# Gerar chave de 32 caracteres para o Aspire
```bash
openssl rand -base64 32
```