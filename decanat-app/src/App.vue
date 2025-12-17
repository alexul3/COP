<template>
  <div id="app">
    <!-- Навигация -->
    <nav class="navbar">
      <div class="container">
        <router-link to="/" class="logo">
          <span class="logo-icon">📚</span>
          <span class="logo-text">Система деканата</span>
        </router-link>
        
        <div class="nav-links">
          <router-link to="/" class="nav-link">Главная</router-link>
          
          <template v-if="auth.isAuthenticated">
            <router-link v-if="auth.isStudent" to="/schedule" class="nav-link">Расписание</router-link>
            <router-link v-if="auth.isStudent" to="/grades" class="nav-link">Оценки</router-link>
            <router-link v-if="auth.isTeacher" to="/teacher/schedule" class="nav-link">Мое расписание</router-link>
            <router-link v-if="auth.isTeacher" to="/teacher/grades" class="nav-link">Журнал оценок</router-link>
            <router-link v-if="auth.isDecanatWorker" to="/decanat/schedules" class="nav-link">Управление расписанием</router-link>
            
            <div class="user-menu">
              <span class="username">{{ auth.user?.username }}</span>
              <span class="user-role">({{ getRoleName(auth.user?.role) }})</span>
              <button class="logout-btn" @click="logout">Выйти</button>
            </div>
          </template>
          
          <template v-else>
            <button class="login-btn" @click="showLogin = true">Войти</button>
            <button class="register-btn" @click="showRegister = true">Регистрация</button>
          </template>
        </div>
      </div>
    </nav>

    <!-- Основной контент -->
    <main class="main-content">
      <div class="container">
        <!-- Отображение ошибок -->
        <div v-if="auth.error" class="alert alert-error">
          {{ auth.error }}
        </div>
        
        <!-- Компонент страницы -->
        <router-view />
      </div>
    </main>

    <!-- Модальное окно входа -->
    <div v-if="showLogin" class="modal-overlay" @click.self="showLogin = false">
      <div class="modal">
        <h3>Вход в систему</h3>
        <form @submit.prevent="handleLogin">
          <div class="form-group">
            <label>Имя пользователя:</label>
            <input v-model="loginForm.username" type="text" required>
          </div>
          <div class="form-group">
            <label>Пароль:</label>
            <input v-model="loginForm.password" type="password" required>
          </div>
          <div class="form-actions">
            <button type="button" @click="showLogin = false">Отмена</button>
            <button type="submit" :disabled="auth.isLoading">
              {{ auth.isLoading ? 'Вход...' : 'Войти' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Модальное окно регистрации -->
    <div v-if="showRegister" class="modal-overlay" @click.self="showRegister = false">
      <div class="modal">
        <h3>Регистрация</h3>
        <form @submit.prevent="handleRegister">
          <div class="form-group">
            <label>Имя пользователя:</label>
            <input v-model="registerForm.username" type="text" required>
          </div>
          <div class="form-group">
            <label>Пароль:</label>
            <input v-model="registerForm.password" type="password" required>
          </div>
          <div class="form-actions">
            <button type="button" @click="showRegister = false">Отмена</button>
            <button type="submit" :disabled="auth.isLoading">
              {{ auth.isLoading ? 'Регистрация...' : 'Зарегистрироваться' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useAuthStore } from '@/Stores/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

const showLogin = ref(false)
const showRegister = ref(false)

const loginForm = reactive({
  username: '',
  password: ''
})

const registerForm = reactive({
  username: '',
  password: ''
})

const getRoleName = (role?: string) => {
  const roles: Record<string, string> = {
    'Student': 'Студент',
    'Teacher': 'Преподаватель',
    'DecanatWorker': 'Работник деканата'
  }
  return role ? roles[role] : ''
}

const handleLogin = async () => {
  try {
    await auth.login(loginForm.username, loginForm.password)
    showLogin.value = false
    loginForm.username = ''
    loginForm.password = ''
    
    // Перенаправление в зависимости от роли
    if (auth.isStudent) {
      await router.push('/schedule')
    } else if (auth.isTeacher) {
      await router.push('/teacher/schedule')
    } else if (auth.isDecanatWorker) {
      await router.push('/decanat/schedules')
    }
  } catch (error) {
    // Ошибка уже обработана в сторе
  }
}

const handleRegister = async () => {
  try {
    await auth.register(registerForm.username, registerForm.password)
    showRegister.value = false
    registerForm.username = ''
    registerForm.password = ''
    showLogin.value = true
  } catch (error) {
    // Ошибка уже обработана в сторе
  }
}

const logout = () => {
  auth.logout()
  router.push('/')
}
</script>

<style scoped>
/* Стили для App.vue */
.navbar {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 1rem 0;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.navbar .container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  color: white;
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 1.5rem;
  font-weight: bold;
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

.user-menu {
  display: flex;
  align-items: center;
  gap: 10px;
  color: white;
}

.username {
  font-weight: bold;
}

.user-role {
  font-size: 0.9rem;
  opacity: 0.8;
}

.login-btn, .register-btn, .logout-btn {
  padding: 0.5rem 1.5rem;
  border-radius: 5px;
  border: none;
  cursor: pointer;
  font-weight: bold;
  transition: transform 0.2s;
}

.login-btn {
  background: #fff;
  color: #667eea;
}

.register-btn {
  background: transparent;
  color: white;
  border: 2px solid white;
}

.logout-btn {
  background: rgba(255,255,255,0.2);
  color: white;
}

.login-btn:hover, .register-btn:hover, .logout-btn:hover {
  transform: translateY(-2px);
}

.main-content {
  padding: 2rem 0;
  min-height: calc(100vh - 200px);
}

.alert {
  padding: 1rem;
  border-radius: 5px;
  margin-bottom: 1rem;
}

.alert-error {
  background: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

/* Модальные окна */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal {
  background: white;
  padding: 2rem;
  border-radius: 10px;
  min-width: 400px;
  max-width: 90%;
}

.modal h3 {
  margin-bottom: 1.5rem;
  color: #333;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #555;
}

.form-group input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}

.form-actions button {
  padding: 0.5rem 1.5rem;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-weight: bold;
}

.form-actions button[type="button"] {
  background: #f8f9fa;
  color: #333;
}

.form-actions button[type="submit"] {
  background: #667eea;
  color: white;
}
</style>