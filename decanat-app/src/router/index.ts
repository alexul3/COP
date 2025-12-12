import { createRouter, createWebHistory } from 'vue-router'

// ИМПОРТИРУЙТЕ КОМПОНЕНТЫ ЯВНО
import HomePage from '../pages/HomePage.vue'
import SchedulePage from '../pages/SchedulePage.vue'
import GradesPage from '../pages/GradesPage.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomePage // Убедитесь, что это не HomePage()
    },
    {
      path: '/schedule',
      name: 'schedule',
      component: SchedulePage
    },
    {
      path: '/grades',
      name: 'grades',
      component: GradesPage
    }
  ]
})

export default router