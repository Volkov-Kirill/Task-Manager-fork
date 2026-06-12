#!/bin/bash
CONFIG_FILE="../secrets.config"

if [ -f "$CONFIG_FILE" ]; then
    DB_NAME=$(grep -oP 'value="\K[^"]+' "$CONFIG_FILE")
else
    echo "Ошибка: Файл secrets.config не найден!"
    exit 1
fi

echo "=== Восстановление системы из нуля ==="

LATEST_BACKUP=$(ls -t ../backups/*.zip 2>/dev/null | head -n 1)

if [ -z "$LATEST_BACKUP" ]; then
    echo "Ошибка: Файлы бэкапов не найдены в папке backups!"
    exit 1
fi

echo "Используется бэкап: $LATEST_BACKUP"

# Удаляем файлы в bin/Debug и Uploads, затем распаковываем бэкап
powershell.exe -Command "
    if (Test-Path '../bin/Debug/$DB_NAME') { Remove-Item '../bin/Debug/$DB_NAME' -Force }
    if (Test-Path '../Uploads') { Remove-Item '../Uploads' -Recurse -Force }
    if (Test-Path '../bin/Debug/logs') { Remove-Item '../bin/Debug/logs' -Recurse -Force }
    
    # Временная распаковка во внешний каталог и раскладка по местам
    Expand-Archive -Path '$LATEST_BACKUP' -DestinationPath '../backups/temp_restore' -Force
    
    if (Test-Path '../backups/temp_restore/$DB_NAME') { Move-Item '../backups/temp_restore/$DB_NAME' '../bin/Debug/' -Force }
    if (Test-Path '../backups/temp_restore/logs') { Move-Item '../backups/temp_restore/logs' '../bin/Debug/' -Force }
    if (Test-Path '../backups/temp_restore/Uploads') { Move-Item '../backups/temp_restore/Uploads' '../' -Force }
    
    Remove-Item '../backups/temp_restore' -Recurse -Force
"

echo "=== Восстановление успешно завершено! Проект реанимирован ==="
