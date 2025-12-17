import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/Stores/auth'

// Импорт компонентов страниц
import HomePage from '@/pages/HomePage.vue'
import SchedulePage from '@/pages/SchedulePage.vue'
import GradesPage from '@/pages/GradesPage.vue'
import DecanatSchedulesPage from '@/pages/DecanatSchedulesPage.vue'
import TeacherGradesPage from '@/pages/TeacherGradesPage.vue'


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomePage,
      meta: { public: true }
    },
    {
      path: '/login',
      name: 'login',
      component: HomePage,
      meta: { public: true }
    },
    {
      path: '/schedule',
      name: 'schedule',
      component: SchedulePage,
      meta: { requiresAuth: true, role: 'Student' }
    },
    {
      path: '/grades',
      name: 'grades',
      component: GradesPage,
      meta: { requiresAuth: true, role: 'Student' }
    },
    {
      path: '/teacher/schedule',
      name: 'teacher-schedule',
      component: SchedulePage,
      meta: { requiresAuth: true, role: 'Teacher' }
    },
    {
      path: '/teacher/grades',
      name: 'teacher-grades',
      component: TeacherGradesPage,
      meta: { requiresAuth: true, role: 'Teacher' }
    },
    {
      path: '/decanat/schedules',
      name: 'decanat-schedules',
      component: DecanatSchedulesPage,
      meta: { requiresAuth: true, role: 'DecanatWorker' }
    }
  ]
})

// Навигационный guard для проверки авторизации
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  // Проверяем, требует ли маршрут аутентификации
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
    return
  }
  
  // Проверяем роль пользователя
  if (to.meta.role) {
    const userRole = authStore.user?.role
    
    if (to.meta.role === 'Student' && !authStore.isStudent) {
      next('/')
      return
    }
    
    if (to.meta.role === 'Teacher' && !authStore.isTeacher) {
      next('/')
      return
    }
    
    if (to.meta.role === 'DecanatWorker' && !authStore.isDecanatWorker) {
      next('/')
      return
    }
  }
  
  next()
})

export default router