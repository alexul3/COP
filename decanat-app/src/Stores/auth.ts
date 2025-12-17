import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import api, { type UserInfo } from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserInfo | null>(api.getCurrentUser())
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!user.value)
  const isStudent = computed(() => user.value?.role === 'Student')
  const isTeacher = computed(() => user.value?.role === 'Teacher')
  const isDecanatWorker = computed(() => user.value?.role === 'DecanatWorker')

  async function login(username: string, password: string) {
    isLoading.value = true
    error.value = null
    
    try {
      user.value = await api.login({ username, password })
      return user.value
    } catch (err: any) {
      error.value = err.response?.data || 'Ошибка входа'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function register(username: string, password: string) {
    isLoading.value = true
    error.value = null
    
    try {
      return await api.register({ username, password })
    } catch (err: any) {
      error.value = err.response?.data || 'Ошибка регистрации'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  function logout() {
    user.value = null
    api.logout()
  }

  return {
    user,
    isLoading,
    error,
    isAuthenticated,
    isStudent,
    isTeacher,
    isDecanatWorker,
    login,
    register,
    logout
  }
})