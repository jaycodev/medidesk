# 🚑 Guía para contribuir al sistema de citas médicas web

Hola equipo 👋  
Este documento explica cómo vamos a trabajar juntos usando **Git y GitHub**. Está pensado para que todos podamos colaborar sin complicaciones.

## 🌱 Flujo de trabajo en equipo

1. Usaremos una sola rama principal llamada `main`, donde irá el código final y funcional.
2. Cada persona trabajará **solo en su propia rama**, que llevará su **nombre** en minúsculas:
   - `jason`
   - `angelo`
   - `andy`
   - `jhuli`
   - `david`
3. Cuando termines una parte, debes subir tus cambios y hacer un **Pull Request (PR)** para que se revise y se una a `main`.

## 🛠️ ¿Cómo trabajar paso a paso?

### 1. Clonar el repositorio (solo la primera vez)

```bash
git clone https://github.com/jason-vila/sistema-citas-medicas.git
cd sistema-citas-medicas
```

### 2. Conectar tu rama remota en local (solo la primera vez)

Las ramas personales ya están creadas en remoto, solo debes traerlas y cambiarte a ella:

```bash
git fetch origin
git checkout -b jason origin/jason
```

Reemplaza `jason` por tu nombre (`angelo`, `andy`, `jhuli`, `david`).

> [!NOTE]
> Solo debes trabajar dentro de tu propia rama.

### 3. Hacer cambios y guardar (commit)

Agrega tus archivos y guarda los cambios con un mensaje claro:

```bash
git add .
git commit -m "Crear vista de login"
```

### 📝 Ejemplos de nombres de commit útiles

- `"Crear formulario de registro de pacientes"`
- `"Agregar validación de fecha en agendamiento"`
- `"Corregir error en login"`
- `"Diseñar interfaz de citas médicas"`
- `"Conectar frontend con API de especialidades"`

Usa verbos en infinitivo (crear, agregar, corregir, etc.) y sé claro.

### 4. Subir tu rama a GitHub

```bash
git push origin jason
```

> Asegúrate de usar tu nombre (ej: `angelo`, `andy`, etc.)

### 5. Abrir un Pull Request

- Entra a GitHub
- Haz clic en el botón para abrir un **Pull Request**
- Explica brevemente qué hiciste
- Espera la revisión de otro compañero o del admin

## 🔄 Mantener tu rama actualizada con `main`

Para evitar conflictos y mantener tu trabajo sincronizado con el proyecto, **es muy importante que, después de que tu PR sea mergeado a `main`, actualices tu rama con los últimos cambios de `main` antes de seguir trabajando o hacer otro PR.**

Puedes hacerlo así:

```bash
git checkout main
git pull origin main

git checkout jason   # reemplaza con tu nombre
git merge main
```

Si hay conflictos, Git te avisará y deberás resolverlos antes de continuar. Esto asegura que tu rama tenga la base más actualizada y evita problemas en futuros PR.

## 📋 Reglas del equipo

- ❌ No trabajes directamente en `main`.
- ✅ Usa solo tu rama personal.
- ✅ Haz commits con mensajes claros y en español.
- ✅ Abre un Pull Request cuando termines una funcionalidad.
- ✅ Revisa los PR de tus compañeros si puedes.
- ✅ Si tienes dudas, pregunta por el grupo.

## 👥 Roles

- Todos: **colaboradores** (pueden subir código en su rama).
- Un integrante será **administrador** para revisar y aceptar los Pull Requests a `main`.

Gracias por tu trabajo y compromiso 🙌  
¡Vamos a construir un sistema médico de calidad entre todos!