<template>
  <div class="home-page">
    <div class="hero">
      <h2>Добро пожаловать в систему деканата!</h2>
      <p>Управляйте расписанием, оценками и учебным процессом</p>
      
      <div class="features">
        <div class="feature-card">
          <div class="feature-icon">📅</div>
          <h3>Расписание</h3>
          <p>Просматривайте и редактируйте расписание занятий</p>
        </div>
        
        <div class="feature-card">
          <div class="feature-icon">📊</div>
          <h3>Оценки</h3>
          <p>Ведение журнала успеваемости студентов</p>
        </div>
        
        <div class="feature-card">
          <div class="feature-icon">👨‍🏫</div>
          <h3>Преподаватели</h3>
          <p>Управление информацией о преподавателях</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.home-page {
  text-align: center;
}

.hero {
  margin-bottom: 3rem;
}

.hero h2 {
  font-size: 2.5rem;
  color: #333;
  margin-bottom: 1rem;
}

.hero p {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 3rem;
}

.features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-top: 2rem;
}

.feature-card {
  background: white;
  padding: 2rem;
  border-radius: 10px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
  transition: transform 0.3s;
}

.feature-card:hover {
  transform: translateY(-5px);
}

.feature-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
}

.feature-card h3 {
  color: #667eea;
  margin-bottom: 1rem;
}
</style>