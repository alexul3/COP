<template>
  <div class="grades-page">
    <h2>Журнал успеваемости</h2>
    
    <div class="grades-summary">
      <div class="summary-card">
        <div class="summary-value">4.8</div>
        <div class="summary-label">Средний балл</div>
      </div>
      <div class="summary-card">
        <div class="summary-value">12</div>
        <div class="summary-label">Всего предметов</div>
      </div>
      <div class="summary-card">
        <div class="summary-value">3</div>
        <div class="summary-label">Зачеты сданы</div>
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
          <tr v-for="subject in subjects" :key="subject.id">
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
              <span class="average-grade">{{ subject.average }}</span>
            </td>
            <td>
              <span :class="['status-badge', subject.status === 'Сдан' ? 'success' : 'warning']">
                {{ subject.status }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface SubjectGrade {
  id: number
  name: string
  teacher: string
  grades: number[]
  average: number
  status: 'Сдан' | 'В процессе'
}

const subjects = ref<SubjectGrade[]>([
  { id: 1, name: 'Программирование', teacher: 'Иванов А.А.', grades: [5, 4, 5, 5], average: 4.75, status: 'Сдан' },
  { id: 2, name: 'Математика', teacher: 'Петрова М.И.', grades: [4, 4, 3, 5], average: 4.0, status: 'В процессе' },
  { id: 3, name: 'Базы данных', teacher: 'Сидоров В.П.', grades: [5, 5, 5], average: 5.0, status: 'Сдан' },
  { id: 4, name: 'Веб-разработка', teacher: 'Козлов Д.С.', grades: [4, 4, 4, 4], average: 4.0, status: 'В процессе' },
  { id: 5, name: 'Алгоритмы', teacher: 'Николаева Е.В.', grades: [5, 5, 4], average: 4.67, status: 'Сдан' },
])

const getGradeClass = (grade: number) => {
  if (grade >= 4) return 'grade-good'
  if (grade === 3) return 'grade-medium'
  return 'grade-bad'
}
</script>

<style scoped>
.grades-page {
  background: white;
  border-radius: 10px;
  padding: 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
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