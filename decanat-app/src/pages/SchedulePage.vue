<template>
  <div class="schedule-page">
    <div class="page-header">
      <h2>Расписание занятий</h2>
      <div class="controls">
        <select v-model="selectedGroup" class="select-input">
          <option value="all">Все группы</option>
          <option value="1">Группа ИВТ-101</option>
          <option value="2">Группа ИВТ-102</option>
        </select>
        
        <input 
          type="date" 
          v-model="selectedDate"
          class="date-input"
        />
      </div>
    </div>

    <div class="schedule-container">
      <table class="schedule-table">
        <thead>
          <tr>
            <th>Время</th>
            <th>Понедельник</th>
            <th>Вторник</th>
            <th>Среда</th>
            <th>Четверг</th>
            <th>Пятница</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="pair in pairs" :key="pair.id">
            <td class="time-cell">
              <strong>{{ pair.time }}</strong>
              <br>
              <small>{{ pair.number }} пара</small>
            </td>
            <td v-for="day in days" :key="day">
              <div v-if="getScheduleForDayAndPair(day, pair.id)" class="lesson-card">
                <div class="lesson-subject">{{ getScheduleForDayAndPair(day, pair.id)?.subject }}</div>
                <div class="lesson-teacher">{{ getScheduleForDayAndPair(day, pair.id)?.teacher }}</div>
                <div class="lesson-room">Ауд: {{ getScheduleForDayAndPair(day, pair.id)?.room }}</div>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="mock-data-info">
      <p>⚠️ Внимание: это демонстрационные данные. Для работы с реальными данными подключите бэкенд.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

interface Lesson {
  id: number
  day: string
  pairId: number
  subject: string
  teacher: string
  room: string
}

const selectedGroup = ref('all')
const selectedDate = ref('')

// Демо-данные
const days = ['Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница']

const pairs = [
  { id: 1, number: '1', time: '8:30 - 10:00' },
  { id: 2, number: '2', time: '10:10 - 11:40' },
  { id: 3, number: '3', time: '12:00 - 13:30' },
  { id: 4, number: '4', time: '14:00 - 15:30' },
  { id: 5, number: '5', time: '15:40 - 17:10' },
]

const scheduleData = ref<Lesson[]>([
  { id: 1, day: 'Понедельник', pairId: 1, subject: 'Программирование', teacher: 'Иванов А.А.', room: '101' },
  { id: 2, day: 'Понедельник', pairId: 2, subject: 'Математика', teacher: 'Петрова М.И.', room: '202' },
  { id: 3, day: 'Вторник', pairId: 1, subject: 'Базы данных', teacher: 'Сидоров В.П.', room: '303' },
  { id: 4, day: 'Среда', pairId: 3, subject: 'Веб-разработка', teacher: 'Козлов Д.С.', room: '404' },
  { id: 5, day: 'Четверг', pairId: 2, subject: 'Алгоритмы', teacher: 'Николаева Е.В.', room: '105' },
  { id: 6, day: 'Пятница', pairId: 4, subject: 'Проектирование', teacher: 'Алексеев К.Н.', room: '206' },
])

const getScheduleForDayAndPair = (day: string, pairId: number) => {
  return scheduleData.value.find(lesson => 
    lesson.day === day && lesson.pairId === pairId
  )
}
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

.controls {
  display: flex;
  gap: 1rem;
}

.select-input, .date-input {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
}

.schedule-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

.schedule-table th {
  background: #667eea;
  color: white;
  padding: 1rem;
  text-align: center;
}

.schedule-table td {
  border: 1px solid #e0e0e0;
  padding: 1rem;
  vertical-align: top;
  min-width: 200px;
  height: 120px;
}

.time-cell {
  background: #f8f9fa;
  text-align: center;
  font-weight: bold;
  color: #333;
}

.lesson-card {
  background: linear-gradient(135deg, #667eea15 0%, #764ba215 100%);
  padding: 0.75rem;
  border-radius: 8px;
  border-left: 4px solid #667eea;
  height: 100%;
}

.lesson-subject {
  font-weight: bold;
  color: #333;
  margin-bottom: 0.5rem;
}

.lesson-teacher {
  color: #666;
  font-size: 0.9rem;
  margin-bottom: 0.25rem;
}

.lesson-room {
  color: #667eea;
  font-size: 0.85rem;
}

.mock-data-info {
  margin-top: 2rem;
  padding: 1rem;
  background: #fff3cd;
  border: 1px solid #ffeaa7;
  border-radius: 5px;
  color: #856404;
  text-align: center;
}
</style>