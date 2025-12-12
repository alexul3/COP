<template>
  <div id="app">
    <nav class="navbar">
      <div class="container">
        <h1 class="logo">📚 Система деканата</h1>
        <div class="nav-links">
          <router-link to="/" class="nav-link">Главная</router-link>
          <router-link to="/schedule" class="nav-link">Расписание</router-link>
          <router-link to="/grades" class="nav-link">Оценки</router-link>
          <router-link to="/teachers" class="nav-link">Преподаватели</router-link>
          <button class="login-btn" @click="simulateLogin">Войти как студент</button>
        </div>
      </div>
    </nav>

    <main class="main-content">
      <div class="container">
        <router-view />
      </div>
    </main>

    <footer class="footer">
      <div class="container">
        <p>© 2024 Система управления деканатом. Все права защищены.</p>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const simulateLogin = () => {
  // Здесь будет логика аутентификации
  localStorage.setItem('userRole', 'student')
  localStorage.setItem('userId', '1')
  alert('Вошли как студент (ID: 1)')
}
</script>

<style scoped>
#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.navbar {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 1rem 0;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.navbar .container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  font-size: 1.5rem;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 10px;
}

.nav-links {
  display: flex;
  gap: 1.5rem;
  align-items: center;
}

.nav-link {
  color: white;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 5px;
  transition: background 0.3s;
}

.nav-link:hover {
  background: rgba(255,255,255,0.1);
}

.nav-link.router-link-active {
  background: rgba(255,255,255,0.2);
  font-weight: bold;
}

.login-btn {
  background: #fff;
  color: #667eea;
  border: none;
  padding: 0.5rem 1.5rem;
  border-radius: 5px;
  cursor: pointer;
  font-weight: bold;
  transition: transform 0.2s;
}

.login-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

.main-content {
  flex: 1;
  padding: 2rem 0;
  background: #f8f9fa;
}

.footer {
  background: #2c3e50;
  color: white;
  text-align: center;
  padding: 1.5rem 0;
  margin-top: auto;
}
</style>