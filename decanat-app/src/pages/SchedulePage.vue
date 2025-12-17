<template>
  <div class="schedule-page">
    <div class="page-header">
      <h2>Мое расписание</h2>
      <div class="controls">
        <input 
          type="date" 
          v-model="selectedDate"
          class="date-input"
        />
      </div>
    </div>

    <div v-if="isLoading" class="loading">
      Загрузка расписания...
    </div>

    <div v-else-if="error" class="error">
      {{ error }}
    </div>

    <div v-else-if="schedules.length === 0" class="empty">
      На этой неделе занятий нет
    </div>

    <div v-else class="schedule-container">
      <div class="day-schedule" v-for="day in groupedSchedules" :key="day.date">
        <h3>{{ formatDate(day.date) }}</h3>
        <div class="lessons">
          <div v-for="lesson in day.lessons" :key="lesson.id" class="lesson-card">
            <div class="lesson-time">
              <span class="pair-number">{{ lesson.pairNumber }} пара</span>
              <span class="time-range">{{ getTimeForPair(lesson.pairNumber) }}</span>
            </div>
            <div class="lesson-details">
              <h4>{{ lesson.subject.name }}</h4>
              <p class="teacher">{{ lesson.teacher.name }}</p>
              <p class="classroom">Аудитория: {{ lesson.classroom }}</p>
              <p class="group" v-if="auth.isDecanatWorker">Группа: {{ lesson.group.name }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/Stores/auth'
import api from '@/services/api'

interface Schedule {
  id: number
  date: string
  pairNumber: number
  classroom: string
  subject: {
    id: number
    name: string
    description: string
  }
  teacher: {
    id: number
    name: string
  }
  group: {
    id: number
    name: string
  }
}

const auth = useAuthStore()
const schedules = ref<Schedule[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)
const selectedDate = ref('')

// Группировка расписания по дням
const groupedSchedules = computed(() => {
  const grouped: Record<string, Schedule[]> = {}
  
  schedules.value.forEach(schedule => {
    if (!grouped[schedule.date]) {
      grouped[schedule.date] = []
    }
    grouped[schedule.date].push(schedule)
  })
  
  // Сортировка по дате и номеру пары
  return Object.entries(grouped)
    .sort(([dateA], [dateB]) => new Date(dateA).getTime() - new Date(dateB).getTime())
    .map(([date, lessons]) => ({
      date,
      lessons: lessons.sort((a, b) => a.pairNumber - b.pairNumber)
    }))
})

const loadSchedule = async () => {
  if (!auth.user) return
  
  isLoading.value = true
  error.value = null
  
  try {
    if (auth.isStudent && auth.user.studentId) {
      schedules.value = await api.getStudentSchedule(auth.user.studentId)
    } else if (auth.isTeacher && auth.user.teacherId) {
      schedules.value = await api.getTeacherSchedule(auth.user.teacherId)
    }
  } catch (err: any) {
    error.value = 'Ошибка загрузки расписания'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}

const formatDate = (dateStr: string) => {
  const date = new Date(dateStr)
  return date.toLocaleDateString('ru-RU', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const getTimeForPair = (pairNumber: number) => {
  const pairs = [
    '8:30 - 10:00',
    '10:10 - 11:40',
    '12:00 - 13:30',
    '14:00 - 15:30',
    '15:40 - 17:10',
    '17:20 - 18:50',
    '19:00 - 20:30'
  ]
  return pairs[pairNumber - 1] || 'Время не указано'
}

onMounted(() => {
  loadSchedule()
})
</script>

<style scoped>
.schedule-page {
  background: white;
  border-radius: 10px;
  padding: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.date-input {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
}

.loading, .error, .empty {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.error {
  color: #e74c3c;
}

.day-schedule {
  margin-bottom: 2rem;
  padding: 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
}

.day-schedule h3 {
  color: #667eea;
  margin-bottom: 1rem;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid #f0f0f0;
}

.lessons {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.lesson-card {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  background: linear-gradient(135deg, #667eea15 0%, #764ba215 100%);
  border-radius: 8px;
  border-left: 4px solid #667eea;
}

.lesson-time {
  min-width: 120px;
  padding-right: 1rem;
  border-right: 1px solid #e0e0e0;
}

.pair-number {
  display: block;
  font-weight: bold;
  color: #333;
}

.time-range {
  display: block;
  font-size: 0.9rem;
  color: #666;
}

.lesson-details {
  flex: 1;
}

.lesson-details h4 {
  color: #333;
  margin-bottom: 0.5rem;
}

.teacher, .classroom, .group {
  margin: 0.25rem 0;
  color: #555;
  font-size: 0.9rem;
}
</style>