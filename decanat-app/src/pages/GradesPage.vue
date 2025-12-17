<template>
  <div class="grades-page">
    <h2>Мои оценки</h2>
    
    <div v-if="isLoading" class="loading">
      Загрузка оценок...
    </div>

    <div v-else-if="error" class="error">
      {{ error }}
    </div>

    <div v-else>
      <div class="grades-summary">
        <div class="summary-card">
          <div class="summary-value">{{ averageGrade }}</div>
          <div class="summary-label">Средний балл</div>
        </div>
        <div class="summary-card">
          <div class="summary-value">{{ totalSubjects }}</div>
          <div class="summary-label">Всего предметов</div>
        </div>
        <div class="summary-card">
          <div class="summary-value">{{ passedSubjects }}</div>
          <div class="summary-label">Сдано предметов</div>
        </div>
      </div>

      <div class="grades-table-container">
        <table class="grades-table">
          <thead>
            <tr>
              <th>Предмет</th>
              <th>Преподаватель</th>
              <th>Оценки</th>
              <th>Средний балл</th>
              <th>Статус</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="subject in groupedGrades" :key="subject.id">
              <td>{{ subject.name }}</td>
              <td>{{ subject.teacher }}</td>
              <td>
                <div class="grades-list">
                  <span 
                    v-for="grade in subject.grades" 
                    :key="grade"
                    :class="['grade-badge', getGradeClass(grade)]"
                  >
                    {{ grade }}
                  </span>
                </div>
              </td>
              <td>
                <span class="average-grade">{{ subject.average.toFixed(2) }}</span>
              </td>
              <td>
                <span :class="['status-badge', subject.average >= 4 ? 'success' : 'warning']">
                  {{ subject.average >= 4 ? 'Сдан' : 'В процессе' }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/Stores/auth'
import api from '@/services/api'

interface Exam {
  id: number
  examScore: number
  subject: {
    id: number
    name: string
  }
  teacher: {
    id: number
    name: string
  }
  student: {
    id: number
    name: string
  }
}

interface SubjectGrade {
  id: number
  name: string
  teacher: string
  grades: number[]
  average: number
}

const auth = useAuthStore()
const exams = ref<Exam[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)

// Вычисляемые свойства
const groupedGrades = computed(() => {
  const subjects: Record<number, SubjectGrade> = {}
  
  exams.value.forEach(exam => {
    if (!subjects[exam.subject.id]) {
      subjects[exam.subject.id] = {
        id: exam.subject.id,
        name: exam.subject.name,
        teacher: exam.teacher.name,
        grades: [],
        average: 0
      }
    }
    subjects[exam.subject.id].grades.push(exam.examScore)
  })
  
  // Рассчитываем средний балл для каждого предмета
  Object.values(subjects).forEach(subject => {
    const sum = subject.grades.reduce((a, b) => a + b, 0)
    subject.average = sum / subject.grades.length
  })
  
  return Object.values(subjects)
})

const averageGrade = computed(() => {
  if (groupedGrades.value.length === 0) return 0
  const sum = groupedGrades.value.reduce((acc, subject) => acc + subject.average, 0)
  return (sum / groupedGrades.value.length).toFixed(2)
})

const totalSubjects = computed(() => groupedGrades.value.length)

const passedSubjects = computed(() => {
  return groupedGrades.value.filter(subject => subject.average >= 4).length
})

// Методы
const loadGrades = async () => {
  if (!auth.user?.studentId) return
  
  isLoading.value = true
  error.value = null
  
  try {
    exams.value = await api.getStudentGrades(auth.user.studentId)
  } catch (err: any) {
    error.value = 'Ошибка загрузки оценок'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}

const getGradeClass = (grade: number) => {
  if (grade >= 4) return 'grade-good'
  if (grade === 3) return 'grade-medium'
  return 'grade-bad'
}

onMounted(() => {
  loadGrades()
})
</script>

<style scoped>
.grades-page {
  background: white;
  border-radius: 10px;
  padding: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.loading, .error {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.error {
  color: #e74c3c;
}

.grades-summary {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin: 2rem 0;
}

.summary-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 1.5rem;
  border-radius: 10px;
  text-align: center;
}

.summary-value {
  font-size: 2.5rem;
  font-weight: bold;
  margin-bottom: 0.5rem;
}

.summary-label {
  font-size: 0.9rem;
  opacity: 0.9;
}

.grades-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

.grades-table th {
  background: #f8f9fa;
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #495057;
  border-bottom: 2px solid #e9ecef;
}

.grades-table td {
  padding: 1rem;
  border-bottom: 1px solid #e9ecef;
}

.grades-list {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.grade-badge {
  display: inline-block;
  width: 30px;
  height: 30px;
  line-height: 30px;
  text-align: center;
  border-radius: 50%;
  font-weight: bold;
}

.grade-good {
  background: #d4edda;
  color: #155724;
}

.grade-medium {
  background: #fff3cd;
  color: #856404;
}

.grade-bad {
  background: #f8d7da;
  color: #721c24;
}

.average-grade {
  font-size: 1.2rem;
  font-weight: bold;
  color: #667eea;
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.status-badge.success {
  background: #d4edda;
  color: #155724;
}

.status-badge.warning {
  background: #fff3cd;
  color: #856404;
}
</style>