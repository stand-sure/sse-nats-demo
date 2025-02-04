#!/usr/bin/env bash

for _ in {1..10}; do
  npx name-creator --output dashed | \
  xargs -n 1 -I {} nats pub foo {};
done

nats pub foo "__END__"
